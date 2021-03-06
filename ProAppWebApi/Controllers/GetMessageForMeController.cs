﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL;

namespace ProAppWebApi.Controllers
{
    public class GetMessageForMeController : ApiController
    {
        public HttpResponseMessage Post([FromBody] String  recNo)
        {
            try
            {
                using (ChatONv1Entities entities = new ChatONv1Entities())
                {
                    var v = entities.Messages.ToList().Where(e => e.Recipient == recNo && e.isRead == false);
                    if (v == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Message for" + recNo + " recipient");
                    }
                    else
                    {
                        
                        int size = 0;
                        foreach (var item in v)
                        {
                            size++;
                        }
                        Message[] arr = new Message[size];
                        int i = 0;
                        foreach (var item in v)
                        {
                            arr[i] = item;
                            i++;
                        }
                        foreach (var item in v)
                        {
                            item.isRead = true;
                        }
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, arr);
                    }
                    
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}
