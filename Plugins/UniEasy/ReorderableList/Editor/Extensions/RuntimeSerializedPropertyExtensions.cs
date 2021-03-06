﻿using System.Reflection;
using System;

namespace UniEasy.Editor
{
    public static class RuntimeSerializedPropertyExtensions
    {
        #region Static Fields

        private const char StopChar = '.';

        #endregion

        #region Static Methods

        public static T GetParent<T>(this RuntimeSerializedProperty property)
        {
            var target = property.RuntimeSerializedObject.Target;
            var length = property.PropertyPath.LastIndexOf(StopChar);
            if (length > -1)
            {
                var path = property.PropertyPath.Substring(0, length);
                target = property.RuntimeSerializedObject.FindProperty(path).Value;
            }
            return (T)target;
        }

        public static Type GetTypeReflection(this RuntimeSerializedProperty property)
        {
            object obj = GetParent<object>(property);
            if (obj == null)
            {
                return null;
            }
            Type type = obj.GetType();
            const BindingFlags bindingFlags = BindingFlags.GetField
                                              | BindingFlags.GetProperty
                                              | BindingFlags.Instance
                                              | BindingFlags.NonPublic
                                              | BindingFlags.Public;
            FieldInfo field = type.GetField(property.Name, bindingFlags);
            if (field == null)
            {
                return null;
            }
            return field.FieldType;
        }

        public static string GetRootPath(this RuntimeSerializedProperty property)
        {
            var rootPath = property.PropertyPath;
            var firstDot = property.PropertyPath.IndexOf(StopChar);
            if (firstDot > 0)
            {
                rootPath = property.PropertyPath.Substring(0, firstDot);
            }
            return rootPath;
        }

        public static object[] GetAttributes<T>(this RuntimeSerializedProperty property)
        {
            object obj = GetParent<object>(property);
            if (obj == null)
            {
                return null;
            }

            Type attrType = typeof(T);
            Type type = obj.GetType();
            const BindingFlags bindingFlags = BindingFlags.GetField
                                              | BindingFlags.GetProperty
                                              | BindingFlags.Instance
                                              | BindingFlags.NonPublic
                                              | BindingFlags.Public;
            FieldInfo field = type.GetField(property.Name, bindingFlags);
            if (field != null)
            {
                return field.GetCustomAttributes(attrType, true);
            }
            return null;
        }

        public static bool HasAttribute<T>(this RuntimeSerializedProperty property)
        {
            object[] attrs = GetAttributes<T>(property);
            if (attrs != null)
            {
                return attrs.Length > 0;
            }
            return false;
        }

        #endregion
    }
}
