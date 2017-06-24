using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Twilio.TwiML;

namespace BabyApi.Controllers
{
    public class PhotosController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var message = new Message();
            message.Body("Enjoy the photo. Hope it makes you smile");
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
            string ngrok = "corey.ngrok.io";
            string folder = "BabyPhotos";
            string backupUrl = @"https://cdn.pixabay.com/photo/2016/04/01/09/26/emote-1299362_1280.png";

            // FIND ONEDRIVE FOLDER
            var userProfilePath = Environment.ExpandEnvironmentVariables("%USERPROFILE%");
            string directory = $"{userProfilePath}\\OneDrive\\{folder}";

            if (!Directory.Exists(directory))
            {
                // IF NO DIRECTORY, RETURN A SAD FACE
                return backupUrl;
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
