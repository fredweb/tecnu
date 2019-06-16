/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 *
 * Este código faz parte do Orchard e é livre para distribuição
 * 
 * 
/****************************************************************************************/


using System;
using System.Web;

namespace XNuvem.Localization
{
    /// <summary>
    ///     An HTML-encoded localized string
    /// </summary>
    public class LocalizedString : MarshalByRefObject, IHtmlString
    {
        public LocalizedString(string languageNeutral)
        {
            Text = languageNeutral;
            TextHint = languageNeutral;
        }

        public LocalizedString(string localized, string scope, string textHint, object[] args)
        {
            Text = localized;
            Scope = scope;
            TextHint = textHint;
            Args = args;
        }

        public string Scope { get; }

        /// <summary>
        ///     The HTML-Encoded original text
        /// </summary>
        public string TextHint { get; }

        public object[] Args { get; }

        /// <summary>
        ///     The HTML-encoded localized text
        /// </summary>
        public string Text { get; }

        string IHtmlString.ToHtmlString()
        {
            return Text;
        }

        public static LocalizedString TextOrDefault(string text, LocalizedString defaultValue)
        {
            if (string.IsNullOrEmpty(text))
                return defaultValue;
            return new LocalizedString(text);
        }

        /// <summary>
        ///     The HTML-encoded localized text
        /// </summary>
        public override string ToString()
        {
            return Text;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            if (Text != null)
                hashCode ^= Text.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var that = (LocalizedString) obj;
            return string.Equals(Text, that.Text);
        }
    }
}