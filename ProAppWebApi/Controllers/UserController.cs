using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL;

namespace ProAppWebApi.Controllers
{
    public class UserController : ApiController
    {
        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            try
            {
                using (ChatONv1Entities entities = new ChatONv1Entities())
                {
                    var v = entities.Users.ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, v);
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
            
        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                using (ChatONv1Entities entities = new ChatONv1Entities())
                {
                    var v = entities.Users.Where(e => e.UserID == id).FirstOrDefault();
                    if (v == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User with id " + id.ToString() + " couldn't be found!!!");


                    return Request.CreateResponse(HttpStatusCode.OK, v);
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] User user)
        {
            try
            {
                using (ChatONv1Entities entities = new ChatONv1Entities())
                {

                    entities.Users.Add(user);
                    entities.SaveChanges();

                    var message = Request.CreateResponse(HttpStatusCode.Created, user);
                    message.Headers.Location = new Uri(Request.RequestUri + user.UserID.ToString());

                    return message;
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }

        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put(int id, [FromBody] User user)
        {
            try
            {
                using (ChatONv1Entities entities = new ChatONv1Entities())
                {
                    var v = entities.Users.Where(e => e.UserID == id).FirstOrDefault();
                    if (v == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User with id " + id.ToString() + " couldn't be found to update!!!");
                    }
                    else
                    {
                        v.UserName = user.UserName;
                        v.Login = user.Login;
                        v.Password = user.Password;
                        
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

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (ChatONv1Entities entities = new ChatONv1Entities())
                {
                    var v = entities.Users.Where(e => e.UserID == id).FirstOrDefault();
                    if (v == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User with id " + id.ToString() + " couldn't be found to delete!!!");
                    }
                    else
                    {
                        entities.Users.Remove(v);
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