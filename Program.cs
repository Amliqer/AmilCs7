using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;


namespace ConsoleApp12
{
    internal class Program
    {
        static void Main(string[] args)
        {

            MyClass myClass = new MyClass();

            myClass.Username = IniFileHelper.GetValue("settings.ini", "General", "Username");
            myClass.Password = IniFileHelper.GetValue("settings.ini", "General", "Password");
            myClass.ConnectionString = IniFileHelper.GetValue("config.ini", "Database", "ConnectionString");


            MySimpleClass mySimpleClass = new MySimpleClass();
            mySimpleClass.MyProperty = 5;

            RangeAttribute rangeAttribute = (RangeAttribute)typeof(MySimpleClass).GetProperty("MyProperty").GetCustomAttributes(typeof(RangeAttribute), false)[0];

            if (mySimpleClass.MyProperty < rangeAttribute.MinValue || mySimpleClass.MyProperty > rangeAttribute.MaxValue)
            {
                Console.WriteLine("Value is out of range");
            }
            else
            {
                Console.WriteLine("Value is within range");
            }
        }
    }
}


[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class IniFileAttribute : Attribute
{
    public string IniFileName { get; set; }
    public string SectionName { get; set; }
    public string KeyName { get; set; }

    public IniFileAttribute(string iniFileName, string sectionName, string keyName)
    {
        IniFileName = iniFileName;
        SectionName = sectionName;
        KeyName = keyName;
    }
}


public static class IniFileHelper
{
    public static string GetValue(string iniFileName, string sectionName, string keyName)
    {
        string value = string.Empty;

        if (File.Exists(iniFileName))
        {
            string[] lines = File.ReadAllLines(iniFileName);

            foreach (string line in lines)
            {
                if (line.StartsWith($"[{sectionName}]"))
                {
                    foreach (string line2 in lines.SkipWhile(l => l != line).Skip(1))
                    {
                        if (line2.StartsWith(keyName + "="))
                        {
                            value = Regex.Match(line2, @"=(.*)").Groups[1].Value.Trim();
                            break;
                        }
                    }
                    break;
                }
            }
        }

        return value;
    }
}

public class MyClass
{
    [IniFile("settings.ini", "General", "Username")]
    public string Username { get; set; }

    [IniFile("settings.ini", "General", "Password")]
    public string Password { get; set; }

    [IniFile("config.ini", "Database", "ConnectionString")]
    public string ConnectionString { get; set; }
}

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class RangeAttribute : Attribute
{
    public int MinValue { get; set; }
    public int MaxValue { get; set; }

    public RangeAttribute(int minValue, int maxValue)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }
}

public class MySimpleClass
{
    [Range(1, 10)]
    public int MyProperty { get; set; }
}
