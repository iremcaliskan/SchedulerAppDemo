﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SchedulerAppDemo.Models;

namespace SchedulerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecurringEventsController : ControllerBase
    {
        private readonly SchedulerContext _context;
        public RecurringEventsController(SchedulerContext context)
        {
            _context = context;
        }

        // GET api/recurringevents
        [HttpGet]
        public IEnumerable<WebAPIRecurring> Get([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            return _context.RecurringEvents
                .Where(e => e.StartDate < to && e.EndDate >= from)
                .ToList()
                .Select(e => (WebAPIRecurring)e);
        }

        // GET api/recurringevents/5
        [HttpGet("{id}")]
        public WebAPIRecurring Get(int id)
        {
            return (WebAPIRecurring)_context
                .RecurringEvents
                .Find(id);
        }

        // POST api/recurringevents
        [HttpPost]
        public ObjectResult Post([FromForm] WebAPIRecurring apiEvent)
        {
            var newEvent = (SchedulerRecurringEvent)apiEvent;

            _context.RecurringEvents.Add(newEvent);
            _context.SaveChanges();

            // delete a single occurrence from  recurring series
            var resultAction = "inserted";
            if (newEvent.RecType == "none")
            {
                resultAction = "deleted";
            }

            return Ok(new
            {
                tid = newEvent.Id,
                action = resultAction
            });
        }

        // PUT api/recurringevents/5
        [HttpPut("{id}")]
        public ObjectResult Put(int id, [FromForm] WebAPIRecurring apiEvent)
        {
            var updatedEvent = (SchedulerRecurringEvent)apiEvent;
            var dbEveht = _context.RecurringEvents.Find(id);
            dbEveht.Name = updatedEvent.Name;
            dbEveht.StartDate = updatedEvent.StartDate;
            dbEveht.EndDate = updatedEvent.EndDate;
            dbEveht.EventPID = updatedEvent.EventPID;
            dbEveht.RecType = updatedEvent.RecType;
            dbEveht.EventLength = updatedEvent.EventLength;

            if (!string.IsNullOrEmpty(updatedEvent.RecType) && updatedEvent.RecType != "none")
            {
                // all modified occurrences must be deleted when we update recurring series

                _context.RecurringEvents.RemoveRange(
                    _context.RecurringEvents.Where(e => e.EventPID == id)
                );
            }

            _context.SaveChanges();

            return Ok(new
            {
                action = "updated"
            });
        }

        // DELETE api/recurringevents/5
        [HttpDelete("{id}")]
        public ObjectResult DeleteEvent(int id)
        {
            var e = _context.RecurringEvents.Find(id);
            if (e != null)
            {
                // some logic specific to recurring events support

                if (e.EventPID != default(int))
                {
                    // deleting modified occurrence from recurring series
                    // If an event with the event_pid value was deleted - it needs updating 
                    // with rec_type==none instead of deleting.

                    e.RecType = "none";
                }
                else
                {
                    // if a recurring series was deleted - delete all modified occurrences of the series
                    if (!string.IsNullOrEmpty(e.RecType) && e.RecType != "none")
                    {
                        // all modified occurrences must be deleted when we update recurring series

                        _context.RecurringEvents.RemoveRange(
                            _context.RecurringEvents.Where(ev => ev.EventPID == id)
                        );
                    }

                    _context.RecurringEvents.Remove(e);
                }


                _context.SaveChanges();
            }

            return Ok(new
            {
                action = "deleted"
            });
        }

    }
}
