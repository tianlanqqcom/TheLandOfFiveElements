/*
 * File: DelegateChecker.cs
 * Description: 用于检查委托是否符合要求
 * Author: tianlan
 * Last update at 24/3/6    15:55
 * 
 * Update Records:
 * tianlan  24/3/6  编写代码主体
 */

using System;

namespace Script.GameFramework.Core
{
    public static class DelegateChecker
    {
        public static bool IsDelegateContainsTargetFunction(Delegate @delegate, string functionName)
        {
            if(@delegate == null)
            {
                return false;
            }

            var methodList = @delegate.GetInvocationList();
            foreach ( var method in methodList )
            {
                if(method.Method.Name == functionName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

