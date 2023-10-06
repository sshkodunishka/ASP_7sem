using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace WebApplication1
{
    public class PostHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            HttpRequest req = context.Request;
            HttpResponse res = context.Response;

            res.ContentType = "application/json";
            res.AppendHeader("Access-Control-Allow-Origin", "*");
            res.AppendHeader("Access-Control-Allow-Credentials", "true");
            res.AppendHeader("Access-Control-Allow-Methods", "*");

            int number;


            if (int.TryParse(req.Params["result"], out number))
            {
                try
                {
                    if (context.Session["SessionData"] == null)
                    {
                        context.Session["SessionData"] = js.Serialize(new
                        {
                            stack = new Stack<int>(new int[] { })
                        });
                    }

                    var SessionData = (string)context.Session["SessionData"];

                    dynamic data = JObject.Parse(SessionData);

                    var array = data.stack;
                    int[] newArray = ((Newtonsoft.Json.Linq.JArray)array).Select(item => (int)item).ToArray();

                    context.Session["SessionData"] = js.Serialize(new
                    {
                        stack = newArray
                    });
                    context.Application.Set("result", number);
                    var response = js.Serialize(new
                    {
                        status = "Success",
                        stack = newArray,
                        result = number
                    });
                    res.Write(response);
                }
                catch (InvalidOperationException)
                {
                    res.Write(js.Serialize(new { status = "Failed" }));
                }
            }
            else
            {
                res.Write(js.Serialize(new { error = new { message = "Type of Params['result'] is not Integer", result = req.Params["result"] } }));
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}