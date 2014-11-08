﻿using System.Web;
using System;
using System.Text;

public static class HttpExtensions
{
    public static Uri AddQuery(this Uri uri, string name, string value)
    {
        var ub = new UriBuilder(uri);
        var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);
        httpValueCollection.Add(name, value);

        if (httpValueCollection.Count == 0)
            ub.Query = String.Empty;
        else
        {
            var sb = new StringBuilder();

            for (int i = 0; i < httpValueCollection.Count; i++)
            {
                string text = httpValueCollection.GetKey(i);
                {
                    text = HttpUtility.UrlEncode(text);

                    string val = (text != null) ? (text + "=") : string.Empty;
                    string[] vals = httpValueCollection.GetValues(i);

                    if (sb.Length > 0)
                        sb.Append('&');

                    if (vals == null || vals.Length == 0)
                        sb.Append(val);
                    else
                    {
                        if (vals.Length == 1)
                        {
                            sb.Append(val);
                            sb.Append(HttpUtility.UrlEncode(vals[0]));
                        }
                        else
                        {
                            for (int j = 0; j < vals.Length; j++)
                            {
                                if (j > 0)
                                    sb.Append('&');

                                sb.Append(val);
                                sb.Append(HttpUtility.UrlEncode(vals[j]));
                            }
                        }
                    }
                }
            }

            ub.Query = sb.ToString();
        }

        return ub.Uri;
    }
}

