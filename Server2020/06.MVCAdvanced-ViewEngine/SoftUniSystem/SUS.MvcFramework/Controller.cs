﻿using SUS.HTTP;
using SUS.MvcFramework.ViewEngine;
using System.Runtime.CompilerServices;
using System.Text;

namespace SUS.MvcFramework
{
    public abstract class Controller
    {
        private SUSViewEngine viewEngine;

        public Controller()
        {
            this.viewEngine = new SUSViewEngine();
        }
        public HttpResponse View(object viewModel = null, [CallerMemberName] string viewPath = null)
        {
            var layout = System.IO.File.ReadAllText("Views/Shared/_Layout.cshtml");
            layout = layout.Replace("@RenderBody()", "___VIEW_GOES_HERE___");
            layout = this.viewEngine.GetHtml(layout, viewModel);

            var viewContent = System.IO.File
                                        .ReadAllText("Views/" +
                                        this.GetType().Name
                                        .Replace("Controller", string.Empty) +
                                        "/" + viewPath +
                                        ".cshtml");

            viewContent = this.viewEngine.GetHtml(viewContent, viewModel);

            var responseHtml = layout.Replace("___VIEW_GOES_HERE___", viewContent);

            var responseBodyBites = Encoding.UTF8.GetBytes(responseHtml);
            var response = new HttpResponse("text/html", responseBodyBites);

            return response;
        }

        public HttpResponse File(string pathFile, string contentType)
        {
            var fileBytes = System.IO.File.ReadAllBytes(pathFile);
            var response = new HttpResponse(contentType, fileBytes);
            return response;
        }

        public HttpResponse Redirect(string url)
        {
            var response = new HttpResponse(HttpStatusCode.Found);
            response.Headers.Add(new Header("Location", url));
            return response;
        }
    }
}