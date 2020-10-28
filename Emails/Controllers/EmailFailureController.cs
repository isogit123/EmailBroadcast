using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emails.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Emails.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmailFailureController : ControllerBase
    {
        ISentEmailsService _sentEmailsService;
        public EmailFailureController(ISentEmailsService sentEmailsService)
        {
            _sentEmailsService = sentEmailsService;
        }

        [HttpPost]
        public async Task Add()
        {
            string jsonObj = "";
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                jsonObj = await reader.ReadToEndAsync();
            }
            object payload = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonObj);
            string email = ((JObject)payload).Value<string>("email");
            string messageId = ((JObject)payload).Value<string>("message-id");
            await _sentEmailsService.AddEmailFailure(new Models.SentEmailsFailures
            {
                Recipient = email,
                SentEmailsId = messageId
            });
        }
    }
}
