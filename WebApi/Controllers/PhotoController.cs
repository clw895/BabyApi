using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using Twilio.TwiML;

namespace WebApi.Controllers
{
    public class PhotoController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var message = new Message();
            message.Body("This is the text of my message");
            message.Media(GetPhotoUrl());

            var response = new MessagingResponse();
            response.Message(message);

            return new HttpResponseMessage
            {
                Content = new StringContent(response.ToString(), System.Text.Encoding.UTF8, "application/xml"),
                StatusCode = HttpStatusCode.OK
            };
        }
        private string GetPhotoUrl()
        {
            string ngrok = ConfigurationManager.AppSettings["NGROK_URL"].ToString();
            string folder = ConfigurationManager.AppSettings["PHOTO_FOLDER"].ToString();
            string backupUrl = @"https://c1.staticflickr.com/3/2899/14341091933_1e92e62d12_b.jpg";

            // FIND ONEDRIVE FOLDER
            var userProfilePath = Environment.ExpandEnvironmentVariables("%USERPROFILE%");
            string directory = $"{userProfilePath}\\OneDrive\\{folder}";

            if (!Directory.Exists(directory))
            {
                // IF NO DIRECTORY, RETURN A SAD FACE
                return "https://cdn.pixabay.com/photo/2016/04/01/09/26/emote-1299362_1280.png";
            }

            // PICK PHOTO
            string photo = SelectPhoto(directory);

            // RETURN URL
            return $@"http://{ngrok}/images/{photo}";
        }

        private string SelectPhoto(string directory)
        {
            var files = Directory.GetFiles(directory);
            int index = new Random().Next(files.Length);
            var temp = files[index].Split('\\');
            return temp[temp.Length - 1];
        }
    }
}
