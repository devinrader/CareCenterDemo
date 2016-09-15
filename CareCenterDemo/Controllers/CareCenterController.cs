using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio.TaskRouter;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;

namespace CareCenterDemo.Controllers
{
    public class CareCenterController : TwilioController
    {
        public ActionResult Message(string body, string AddOns)
        {
            var response = new TwilioResponse();

            dynamic addons = JsonConvert.DeserializeObject(AddOns);

            if (addons["status"] == "successful")
            {
                var language = addons["results"]["ibm_watson_insights"]["result"]["language"].Value;

                if (language == "spanish")
                {
                    response.Message("Gracias por contactar con nuestro equipo de soporte. Uno de nuestros agentes de soporte estará con usted pronto.");
                }
                else if (language == "french")
                {
                    response.Message("Merci de contacter notre équipe d’assistance. Un de nos agents de soutien fera avec vous sous peu.");
                }
                else
                {
                    response.Message("Thank you for contacting our support team.  One of our support agents will be with you shortly.");
                }

                var client = new TaskRouterClient("YOUR_ACCOUNT_SID", "YOUR_AUTH_TOKEN");
                var result = client.AddTask(
                    "YOUR_WORKSPACE_SID",
                    JsonConvert.SerializeObject(new { language = language, body = body }),
                    "YOUR_WORKFLOW_SID",
                    null, null);

                if (result.RestException != null)
                {
                    throw new HttpException(result.RestException.Message);
                }
            }
            else
            {
                response.Message("We're really sorry but something seems to have gone wrong when we tried to read your message.  Can you try sending it again?  If it keeps happening us, try giving us a call at 1-800-555-5555");
            }

            return TwiML(response);
        }
    }
}