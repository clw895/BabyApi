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
    public class PhotosController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get()
        {
            Message message = new Message();
            message.Body("This is the text of my message");
            message.Media(GetPhotoUrl());

            MessagingResponse response = new MessagingResponse();
            response.Message(message);

            return new HttpResponseMessage
            {
                Content = new StringContent(response.ToString(), System.Text.Encoding.UTF8, "application/xml"),
                StatusCode = HttpStatusCode.OK
            };
        }
        private string GetPhotoUrl()
        {
            const string ngrok = ConfigurationManager.AppSettings["NGROK_URL"].ToString();
            const string folder = ConfigurationManager.AppSettings["PHOTO_FOLDER"].ToString();
            const string backupUrl = @"https://c1.staticflickr.com/3/2899/14341091933_1e92e62d12_b.jpg";

            // FIND ONEDRIVE FOLDER
            const string userProfilePath = Environment.ExpandEnvironmentVariables("%USERPROFILE%");
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
            string[] files = Directory.GetFiles(directory);
            int index = new Random().Next(files.Length);
            string[] temp = files[index].Split('\\');
            return temp[temp.Length - 1];
        }
    }
}
