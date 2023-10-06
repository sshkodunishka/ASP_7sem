using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace WebApplication1
{
    public class PutHandler : IHttpHandler, IRequiresSessionState
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
            var strAdd = req.Params["add"];
            Console.WriteLine(strAdd);
            if (int.TryParse(req.Params["add"], out number))
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
                    int result = Convert.ToInt32(context.Application.Get("result"));
                    
                    var array = data.stack;
                    int[] newArray = ((Newtonsoft.Json.Linq.JArray)array).Select(item => (int)item).ToArray();
                    Stack<int> stack = new Stack<int>(new int[] { });
                    foreach (int i in newArray.Reverse())
                    {
                        stack.Push(Convert.ToInt32(i));
                    }
                    stack.Push(Convert.ToInt32(number));

                    context.Session["SessionData"] = js.Serialize(new
                    {
                        stack
                    });

                    var response = js.Serialize(new
                    {
                        status = "Success",
                        result,
                        stack
                    }); ;
                    res.Write(response);
                }
                catch (InvalidOperationException)
                {
                    res.Write(js.Serialize(new { status = "Failed"}));
                }
            }
            else
            {
                res.Write(js.Serialize(new { error = new { message = "Type of Params['add'] is not Integer", result = req.Params["add"] } }));
            }

        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}