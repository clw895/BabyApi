using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
            message.Body("Enjoy the photo. I hope it makes you smile");

            var urlTask = GetPhotoUrl();
            urlTask.Wait();
            message.Media(urlTask.Result);

            var response = new MessagingResponse();
            response.Message(message);

            return new HttpResponseMessage
            {
                Content = new StringContent(response.ToString(), System.Text.Encoding.UTF8, "application/xml"),
                StatusCode = HttpStatusCode.OK
            };
        }
        private async Task<string> GetPhotoUrl()
        {
            var backupUrl = @"https://cdn.pixabay.com/photo/2016/04/01/09/26/emote-1299362_1280.png";

            // GET DROPBOX FOLDERS
            string photoUrl = String.Empty;
            try
            {
                // CREATE DROPBOX HELPER
                var helper = new DropboxHelper();

                // GET ALL PHOTO PATHS
                var filePaths = Task.Run(()=>helper.GetFilePaths()).Result;

                // SELECT RANDOM FILE
                var selectedPath = SelectRandomFilePath(filePaths);

                // GET PUBLICLY ACCESSIBLE URI
                photoUrl = Task.Run(()=> helper.GetFileUri(selectedPath)).Result;
             }
            catch
            {
                photoUrl = backupUrl;
            }

            return photoUrl;
        }

        private string SelectRandomFilePath(List<string> filePaths)
        {
            int index = new Random().Next(filePaths.Count);
            return filePaths[index];
        }
    }
}
