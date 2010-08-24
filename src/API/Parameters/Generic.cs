using System;
using System.Reflection;

namespace Twentythree
{
#region Enumeration helpers
    public class RequestValue : Attribute
    {
        public string Value;

        public RequestValue(string AValue)
        {
            Value = AValue;
        }
    }

    public class RequestValues
    {
        public static string Get(Enum Enumerator)
        {
            Type EnumeratorType = Enumerator.GetType();
            MemberInfo[] MemberInfo = EnumeratorType.GetMember(Enumerator.ToString());

            if ((MemberInfo != null) && (MemberInfo.Length > 0))
            {
                object[] MemberAttributes = MemberInfo[0].GetCustomAttributes(typeof(RequestValue), false);
                if ((MemberAttributes != null) && (MemberAttributes.Length > 0)) return ((RequestValue)MemberAttributes[0]).Value;
            }

            return Enumerator.ToString();
        }
    }
#endregion

    public enum GenericSort
    {
        [RequestValue("asc")]
        Ascending,
        [RequestValue("desc")]
        Descending
    }

    public enum CommentObjectType
    {
        [RequestValue("")]
        Empty,
        [RequestValue("photo")]
        Photo,
        [RequestValue("album")]
        Album
    }
}