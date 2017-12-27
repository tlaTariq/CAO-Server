using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL;

namespace ProAppWebApi.Controllers
{
    public class ConnectedUsersController : ApiController
    {
        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            try
            {
                using (ChatONv1Entities entities = new ChatONv1Entities())
                {
                    var v = entities.ConnectedUsers.ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, v);
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }

        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(string login)
        {
            try
            {
                using (ChatONv1Entities entities = new ChatONv1Entities())
                {
                    var v = entities.ConnectedUsers.Where(e => e.Login == login).FirstOrDefault();
                    if (v == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User with login " + login + " couldn't be found!!!");


                    return Request.CreateResponse(HttpStatusCode.OK, v);
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] ConnectedUser user)
        {
            try
            {
                using (ChatONv1Entities entities = new ChatONv1Entities())
                {

                    entities.ConnectedUsers.Add(user);
                    entities.SaveChanges();

                    var message = Request.CreateResponse(HttpStatusCode.Created, user);
                    message.Headers.Location = new Uri(Request.RequestUri + user.Login.ToString());

                    return message;
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }

        }

        // PUT api/<controller>/5
        //public HttpResponseMessage Put(int id, [FromBody] Message msg)
        //{
        //    try
        //    {
        //        using (ChatONv1Entities entities = new ChatONv1Entities())
        //        {
        //            var v = entities.Messages.Where(e => e.MessageID == id).FirstOrDefault();
        //            if (v == null)
        //            {
        //                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Message with id " + id.ToString() + " couldn't be found to update!!!");
        //            }
        //            else
        //            {
        //                v.Sender = msg.Sender;
        //                v.Recipient = msg.Recipient;
        //                v.Time = msg.Time;
        //                v.Message1 = msg.Message1;
        //                v.isRead = msg.isRead;

        //                entities.SaveChanges();
        //                return Request.CreateResponse(HttpStatusCode.OK, v);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
        //    }
        //}

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (ChatONv1Entities entities = new ChatONv1Entities())
                {
                    var v = entities.Messages.Where(e => e.MessageID == id).FirstOrDefault();
                    if (v == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Message with id " + id.ToString() + " couldn't be found to delete!!!");
                    }
                    else
                    {
                        entities.Messages.Remove(v);
                        entities.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK, v);
                    }
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }
    }
}