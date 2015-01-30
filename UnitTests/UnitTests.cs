﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FiddlerCSP
{
    [TestClass]
    public class UnitTests
    {
        private class ConsoleLogger : ILogger
        {
            public void Log(string message)
            {
                Console.Out.WriteLine("FiddlerCSP: " + message);
            }
        };
        private ILogger logger = new ConsoleLogger();

        private void ValidateCSPReportSet(
            string documentUri,
            string expectedCsp,
            string[] cspReportsUnsafeInline,
            string[] cspReportsUnsafeEval)
        {
            CSPRuleCollector collector = new CSPRuleCollector(logger);

            foreach (var cspReport in cspReportsUnsafeInline.Select(cspReportAsString => CSPReport.Parse(cspReportAsString)))
            {
                collector.Add(cspReport, CSPRuleCollector.InterpretBlank.UnsafeInline);
            }
            foreach (var cspReport in cspReportsUnsafeEval.Select(cspReportAsString => CSPReport.Parse(cspReportAsString)))
            {
                collector.Add(cspReport, CSPRuleCollector.InterpretBlank.UnsafeEval);
            }

            Assert.AreEqual(collector.Get(documentUri), expectedCsp);
        }

        [TestMethod]
        public void TestFirefox35_0_1_httpsStatusModernIE()
        {
            // Interesting notes:
            //  CSP <http://www.w3.org/TR/CSP/#report-uri>
            //  CSP <https://developer.mozilla.org/en-US/docs/Web/Security/CSP/Using_CSP_violation_reports>
            //  blocked-uri property can be 'self' which doesn't match spec or MDN
            //  script-sample property contains a snippet of code
            //  source-file property from CSP2 but not line or column
            //  Weird issue with two violated directives: \"violated-directive\":\"script-src 'unsafe-eval'script-src 'unsafe-inline'
            ValidateCSPReportSet(
                "https://status.modern.ie/",
                "Content-Security-Policy: default-src 'none'; connect-src dc.services.visualstudio.com 'self' www.chromestatus.com; img-src 'self' ssl.google-analytics.com; script-src az416426.vo.msecnd.net 'self' www.google-analytics.com; style-src 'self'",
                new string[] {
                    "{\"csp-report\":{\"blocked-uri\":\"https://status.modern.ie/styles/c75c186a.main.css\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"violated-directive\":\"style-src 'none'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"self\",\"document-uri\":\"https://status.modern.ie/\",\"line-number\":1,\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"script-sample\":\"(function () {\\n            if (navigator...\",\"source-file\":\"https://status.modern.ie/\",\"violated-directive\":\"script-src 'unsafe-eval'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"self\",\"document-uri\":\"https://status.modern.ie/\",\"line-number\":10,\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"script-sample\":\"window.appInsights={queue:[],application...\",\"source-file\":\"https://status.modern.ie/\",\"violated-directive\":\"script-src 'unsafe-eval'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://az416426.vo.msecnd.net/scripts/a/ai.0.7.js\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-eval'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"self\",\"document-uri\":\"https://status.modern.ie/\",\"line-number\":13,\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"script-sample\":\"var _gaq = [\\n        ['_setAccount', 'UA...\",\"source-file\":\"https://status.modern.ie/\",\"violated-directive\":\"script-src 'unsafe-eval'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://www.google-analytics.com/ga.js\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-eval'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://status.modern.ie/scripts/044a95db.vendor.js\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-eval'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://status.modern.ie/scripts/0ba0e107.scripts.js\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-eval'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://status.modern.ie/images/655d5971.ie-logo.png\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"violated-directive\":\"img-src 'none'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://status.modern.ie/images/82999546.top-header-sprite.min.svg\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"violated-directive\":\"img-src 'none'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://status.modern.ie/images/dist/9b2b6e5d.spritesheet.png\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"violated-directive\":\"img-src 'none'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://dc.services.visualstudio.com/v2/track\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"violated-directive\":\"connect-src 'none'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"self\",\"document-uri\":\"https://status.modern.ie/\",\"line-number\":1,\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"script-sample\":\"@charset \\\"UTF-8\\\";[ng:cloak],[ng-cloak],...\",\"source-file\":\"https://status.modern.ie/\",\"violated-directive\":\"style-src 'none'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://status.modern.ie/features\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"violated-directive\":\"connect-src 'none'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://status.modern.ie/images/f240132a.little-arrow.svg\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"violated-directive\":\"img-src 'none'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://www.chromestatus.com/features.json\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"violated-directive\":\"connect-src 'none'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://status.modern.ie/uservoice\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"violated-directive\":\"connect-src 'none'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://status.modern.ie/images/ca0668eb.arrow.svg\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"violated-directive\":\"img-src 'none'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://ssl.google-analytics.com/r/__utm.gif?utmwv=5.6.2&amp;utms=1&amp;utmn=395764756&amp;utmhn=status.modern.ie&amp;utmcs=UTF-8&amp;utmsr=1440x960&amp;utmvp=1423x823&amp;utmsc=24-bit&amp;utmul=en-us&amp;utmje=0&amp;utmfl=-&amp;utmdt=Internet%20Explorer%20Web%20Platform%20Status%20and%20Roadmap%20-%20status.modern.IE&amp;utmhid=1813362053&amp;utmr=-&amp;utmp=%2Fstatus%2F&amp;utmht=1422425247451&amp;utmac=UA-37396709-1&amp;utmcc=__utma%3D50041018.209687841.1422230123.1422338592.1422425246.4%3B%2B__utmz%3D50041018.1422230123.1.1.utmcsr%3D(direct)%7Cutmccn%3D(direct)%7Cutmcmd%3D(none)%3B&amp;utmjid=418976018&amp;utmredir=1&amp;utmu=qhCAAAAAAAAAAAAAAAAAAAAE~\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"referrer\":\"\",\"violated-directive\":\"img-src 'none'\"}}"
                },
                new string[] {
                    "{\"csp-report\":{\"blocked-uri\":\"https://az416426.vo.msecnd.net/scripts/a/ai.0.7.js\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"script-src 'unsafe-inline'; report-uri https://fiddlercsp.deletethis.net/unsafe-eval\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-eval'script-src 'unsafe-inline'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://www.google-analytics.com/ga.js\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"script-src 'unsafe-inline'; report-uri https://fiddlercsp.deletethis.net/unsafe-eval\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-eval'script-src 'unsafe-inline'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://status.modern.ie/scripts/044a95db.vendor.js\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"script-src 'unsafe-inline'; report-uri https://fiddlercsp.deletethis.net/unsafe-eval\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-eval'script-src 'unsafe-inline'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"https://status.modern.ie/scripts/0ba0e107.scripts.js\",\"document-uri\":\"https://status.modern.ie/\",\"original-policy\":\"script-src 'unsafe-inline'; report-uri https://fiddlercsp.deletethis.net/unsafe-eval\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-eval'script-src 'unsafe-inline'\"}}",
                    "{\"csp-report\":{\"blocked-uri\":\"self\",\"document-uri\":\"https://status.modern.ie/\",\"line-number\":25,\"original-policy\":\"script-src 'unsafe-inline'; report-uri https://fiddlercsp.deletethis.net/unsafe-eval\",\"referrer\":\"\",\"script-sample\":\"call to eval() or related function blocked by CSP\",\"source-file\":\"https://status.modern.ie/scripts/044a95db.vendor.js\",\"violated-directive\":\"script-src 'unsafe-inline'\"}}",
                });
        }

        [TestMethod]
        public void TestChrome40_0_2214_93_httpsStatusModernIE()
        {
            // Interesting notes:
            //  CSP L2 <http://www.w3.org/TR/CSP11/>
            ValidateCSPReportSet(
                "https://status.modern.ie/", 
                "Content-Security-Policy: default-src 'none'; connect-src dc.services.visualstudio.com 'self' www.chromestatus.com; font-src www.modern.ie; img-src 'self' ssl.google-analytics.com; script-src az416426.vo.msecnd.net 'self' 'unsafe-eval' 'unsafe-inline' www.google-analytics.com; style-src 'self' 'unsafe-inline'",
                new string[] { 
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"img-src 'none'\",\"effective-directive\":\"img-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://status.modern.ie/images/655d5971.ie-logo.png\",\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"font-src 'none'\",\"effective-directive\":\"font-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://www.modern.ie\",\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"style-src 'none'\",\"effective-directive\":\"style-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://status.modern.ie/styles/c75c186a.main.css\",\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"img-src 'none'\",\"effective-directive\":\"img-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://status.modern.ie/images/dist/9b2b6e5d.spritesheet.png\",\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"img-src 'none'\",\"effective-directive\":\"img-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://status.modern.ie/images/82999546.top-header-sprite.min.svg\",\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"connect-src 'none'\",\"effective-directive\":\"connect-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://dc.services.visualstudio.com\",\"source-file\":\"https://az416426.vo.msecnd.net\",\"line-number\":1,\"column-number\":9872,\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"style-src 'none'\",\"effective-directive\":\"style-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"\",\"source-file\":\"https://status.modern.ie/scripts/044a95db.vendor.js\",\"line-number\":26,\"column-number\":2375,\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"img-src 'none'\",\"effective-directive\":\"img-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://status.modern.ie/images/f240132a.little-arrow.svg\",\"source-file\":\"https://status.modern.ie/scripts/0ba0e107.scripts.js\",\"line-number\":1,\"column-number\":15465,\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"connect-src 'none'\",\"effective-directive\":\"connect-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://status.modern.ie/features\",\"source-file\":\"https://status.modern.ie/scripts/044a95db.vendor.js\",\"line-number\":25,\"column-number\":982,\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"connect-src 'none'\",\"effective-directive\":\"connect-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://www.chromestatus.com\",\"source-file\":\"https://status.modern.ie/scripts/044a95db.vendor.js\",\"line-number\":25,\"column-number\":982,\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"connect-src 'none'\",\"effective-directive\":\"connect-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://status.modern.ie/uservoice\",\"source-file\":\"https://status.modern.ie/scripts/044a95db.vendor.js\",\"line-number\":25,\"column-number\":982,\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"img-src 'none'\",\"effective-directive\":\"img-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://status.modern.ie/images/dist/9b2b6e5d.spritesheet.png\",\"source-file\":\"https://status.modern.ie/scripts/0ba0e107.scripts.js\",\"line-number\":1,\"column-number\":7884,\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"img-src 'none'\",\"effective-directive\":\"img-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://status.modern.ie/images/ca0668eb.arrow.svg\",\"source-file\":\"https://status.modern.ie/scripts/0ba0e107.scripts.js\",\"line-number\":1,\"column-number\":7884,\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"img-src 'none'\",\"effective-directive\":\"img-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://ssl.google-analytics.com\",\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-eval'\",\"effective-directive\":\"script-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://az416426.vo.msecnd.net\",\"source-file\":\"https://status.modern.ie/\",\"line-number\":10,\"column-number\":460,\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-eval'\",\"effective-directive\":\"script-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"\",\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-eval'\",\"effective-directive\":\"script-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://www.google-analytics.com\",\"source-file\":\"https://status.modern.ie/\",\"line-number\":22,\"column-number\":22,\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-eval'\",\"effective-directive\":\"script-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://status.modern.ie/scripts/0ba0e107.scripts.js\",\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-eval'\",\"effective-directive\":\"script-src\",\"original-policy\":\"child-src 'none'; connect-src 'none'; font-src 'none'; frame-src 'none'; img-src 'none'; media-src 'none'; object-src 'none'; style-src 'none'; script-src 'unsafe-eval'; report-uri https://fiddlercsp.deletethis.net/unsafe-inline\",\"blocked-uri\":\"https://status.modern.ie/scripts/044a95db.vendor.js\",\"status-code\":0}}",
                }, new string[] {
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-inline'\",\"effective-directive\":\"script-src\",\"original-policy\":\" script-src 'unsafe-inline'; report-uri https://fiddlercsp.deletethis.net/unsafe-eval\",\"blocked-uri\":\"\",\"source-file\":\"https://status.modern.ie/scripts/044a95db.vendor.js\",\"line-number\":25,\"column-number\":9976,\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-inline'\",\"effective-directive\":\"script-src\",\"original-policy\":\" script-src 'unsafe-inline'; report-uri https://fiddlercsp.deletethis.net/unsafe-eval\",\"blocked-uri\":\"https://az416426.vo.msecnd.net\",\"source-file\":\"https://status.modern.ie/\",\"line-number\":10,\"column-number\":460,\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-inline'\",\"effective-directive\":\"script-src\",\"original-policy\":\" script-src 'unsafe-inline'; report-uri https://fiddlercsp.deletethis.net/unsafe-eval\",\"blocked-uri\":\"https://www.google-analytics.com\",\"source-file\":\"https://status.modern.ie/\",\"line-number\":22,\"column-number\":22,\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-inline'\",\"effective-directive\":\"script-src\",\"original-policy\":\" script-src 'unsafe-inline'; report-uri https://fiddlercsp.deletethis.net/unsafe-eval\",\"blocked-uri\":\"https://status.modern.ie/scripts/044a95db.vendor.js\",\"status-code\":0}}",
                    "{\"csp-report\":{\"document-uri\":\"https://status.modern.ie/\",\"referrer\":\"\",\"violated-directive\":\"script-src 'unsafe-inline'\",\"effective-directive\":\"script-src\",\"original-policy\":\" script-src 'unsafe-inline'; report-uri https://fiddlercsp.deletethis.net/unsafe-eval\",\"blocked-uri\":\"https://status.modern.ie/scripts/0ba0e107.scripts.js\",\"status-code\":0}}",
                });
        }
    }
}
