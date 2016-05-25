using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Android.OS;
using EsseMimmo.Views;
using EsseMimmo.Models;

namespace EsseMimmo
{
    [Activity(Label = "iEsseMimmo", MainLauncher = true)]

    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);


            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Button button = FindViewById<Button>(Resource.Id.myButton);
            //button.Click += delegate {

            //    var email = new Intent(Android.Content.Intent.ActionSend);
            //    email.PutExtra(Android.Content.Intent.ExtraEmail, new string[] { "person1@xamarin.com", "person2@xamrin.com" });
            //    email.PutExtra(Android.Content.Intent.ExtraCc, new string[] { "person3@xamarin.com" });
            //    email.PutExtra(Android.Content.Intent.ExtraSubject, "Hello Email");
            //    email.PutExtra(Android.Content.Intent.ExtraText, "Hello from Xamarin.Android!");
            //    email.SetType("message/rfc822");
            //    StartActivity(email);
            //};


            var webView = FindViewById<WebView>(Resource.Id.webView);
            webView.Settings.JavaScriptEnabled = true;

            // Use subclassed WebViewClient to intercept hybrid native calls
            webView.SetWebViewClient(new HybridWebViewClient());

            // Render the view from the type generated from RazorView.cshtml
            var model = new Model1() { Text = "Text goes here" };
            var template = new RazorView() { Model = model };
            var page = template.GenerateString();

            // Load the rendered HTML into the view with a base URL 
            // that points to the root of the bundled Assets folder
            webView.LoadDataWithBaseURL("file:///android_asset/", page, "text/html", "UTF-8", null);

        }

        private class HybridWebViewClient : WebViewClient
        {
            public override bool ShouldOverrideUrlLoading(WebView webView, string url)
            {

                var scheme = "hybrid:";

                if (!url.StartsWith(scheme))
                    return false;

                var resources = url.Substring(scheme.Length).Split('?');
                var method = resources[0];
                var parameters = System.Web.HttpUtility.ParseQueryString(resources[1]);

                if (method == "UpdateLabel")
                {
                    var subject = "Ordine di " + parameters["nome"];
                    var from = parameters["email"];
                    var body = parameters["menu"];
                    var to = "dave0bit@hotmail.com";

                    var email = new Intent(Android.Content.Intent.ActionSend);
                    email.PutExtra(Android.Content.Intent.ExtraEmail, new string[] { to });
                    email.PutExtra(Android.Content.Intent.ExtraCc, new string[] { from });
                    email.PutExtra(Android.Content.Intent.ExtraSubject, subject);
                    email.PutExtra(Android.Content.Intent.ExtraText, body);
                    email.SetType("message/rfc822");

                    webView.Context.StartActivity(Intent.CreateChooser(email, "Send Email Using: "));

                    var js = string.Format("SetLabelText('{0}');", "Ordine inviato");
                    webView.LoadUrl("javascript:" + js);

                }
                return true;
            }
        }
    }
}

