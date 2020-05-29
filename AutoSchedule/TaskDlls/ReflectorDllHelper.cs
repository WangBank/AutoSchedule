using AutoSchedule.Common;
using ExcuteInterface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoSchedule.TaskDlls
{
    public static class ReflectorDllHelper
    {
        public static Object ReturnObjType(string dllPath,string DllOrUrl,  JobLogger _jobLogger)
        {
            Assembly assembly = Assembly.LoadFrom(dllPath);
            Type type = assembly.GetType(DllOrUrl.Split(',')[1]);
            object[] parameters = new object[1];
            parameters[0] = _jobLogger;
            object obj = Activator.CreateInstance(type, parameters);
            return obj;
        }

        public static Object ReturnObjType(string dllPath, string DllOrUrl)
        {
            Assembly assembly = Assembly.LoadFrom(dllPath);
            Type type = assembly.GetType(DllOrUrl.Split(',')[1]);
            object obj = Activator.CreateInstance(type);
            return obj;
        }

    }
}
