﻿using JinEnergy.Engine;
using Microsoft.JSInterop;
using Shared.Engine.SISI;

namespace JinEnergy.SISI
{
    public class EpornerController : BaseController
    {
        [JSInvokable("epr")]
        async public static ValueTask<dynamic> Index(string args)
        {
            string? search = parse_arg("search", args);
            string? sort = parse_arg("sort", args);
            string? c = parse_arg("c", args);
            int pg = int.Parse(parse_arg("pg", args) ?? "1") + 1;

            string? html = await EpornerTo.InvokeHtml(AppInit.Eporner.corsHost(), search, sort, c, pg, url => JsHttpClient.Get(url));
            if (html == null)
                return OnError("html");

            return OnResult(EpornerTo.Playlist("epr/vidosik", html), EpornerTo.Menu(null, sort, c));
        }


        [JSInvokable("epr/vidosik")]
        async public static ValueTask<dynamic> Stream(string args)
        {
            var stream_links = await EpornerTo.StreamLinks("epr/vidosik", AppInit.Eporner.corsHost(), parse_arg("uri", args), 
                               htmlurl => JsHttpClient.Get(htmlurl), 
                               jsonurl => JsHttpClient.Get(AppInit.Eporner.corsHost(jsonurl)));

            if (stream_links == null)
                return OnError("stream_links");

            return stream_links;
        }
    }
}
