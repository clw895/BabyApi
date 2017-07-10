using System;
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
        public async Task<HttpResponseMessage> Get()
        {
            var message = new Message();
            message.Body("Enjoy the photo. I hope it makes you smile");

            var urlTask =  await GetPhotoUrl();
            message.Media(urlTask);

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
            var photoUrl = string.Empty;
            try
            {
                // CREATE DROPBOX HELPER
                var helper = new DropboxHelper();

                // GET ALL PHOTO PATHS
                var filePaths = await helper.GetFilePaths();

                // SELECT RANDOM FILE
                var index = new Random().Next(filePaths.Count);

                // GET PUBLICLY ACCESSIBLE URI
                photoUrl = await helper.GetFileUri(filePaths[index]);
             }
            catch
            {
                // If anything fails, send a sad face
                photoUrl = backupUrl;
            }

            return photoUrl;
        }
    }
}
