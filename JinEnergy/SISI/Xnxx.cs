﻿using JinEnergy.Engine;
using JinEnergy.Model;
using Microsoft.JSInterop;
using Shared.Engine.SISI;

namespace JinEnergy.SISI
{
    public class XnxxController : BaseController
    {
        [JSInvokable("xnx")]
        async public static ValueTask<ResultModel> Index(string args)
        {
            var init = AppInit.Xnxx.Clone();

            string? search = parse_arg("search", args);
            int pg = int.Parse(parse_arg("pg", args) ?? "1");

            refresh: string? html = await XnxxTo.InvokeHtml(init.corsHost(), search, pg, url => JsHttpClient.Get(init.cors(url)));

            var playlist = XnxxTo.Playlist("xnx/vidosik", html, pl =>
            {
                pl.picture = rsizehost(pl.picture);
                return pl;
            });

            if (playlist.Count == 0)
            {
                if (IsRefresh(init, true))
                    goto refresh;

                return OnError("playlist");
            }

            return OnResult(XnxxTo.Menu(null), playlist);
        }


        [JSInvokable("xnx/vidosik")]
        async public static ValueTask<ResultModel> Stream(string args)
        {
            var init = AppInit.Xnxx.Clone();

            refresh: var stream_links = await XnxxTo.StreamLinks("xnx/vidosik", init.corsHost(), parse_arg("uri", args), url => JsHttpClient.Get(init.cors(url)), url => JsHttpClient.Get(init.cors(url)));

            if (stream_links == null)
            {
                if (IsRefresh(init, true))
                    goto refresh;

                return OnError("stream_links");
            }

            return OnResult(init, stream_links);
        }
    }
}
