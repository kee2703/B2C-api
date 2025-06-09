using Microsoft.AspNetCore.Mvc;
using RotaryAdminAPI.Models;
using RotaryAdminAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System;

namespace RotaryAdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventService _eventService;

        public EventController(EventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_eventService.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var ev = _eventService.GetById(id);
            return ev == null ? NotFound() : Ok(ev);
        }

        [HttpPost]
        public IActionResult Create(Event newEvent)
        {
            var ev = _eventService.Add(newEvent);
            return CreatedAtAction(nameof(GetById), new { id = ev.Id }, ev);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Event updated)
        {
            var result = _eventService.Update(id, updated);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _eventService.Delete(id);
            return result ? NoContent() : NotFound();
        }

        [HttpGet("getByDate")]
        public IActionResult GetByDate([FromQuery] DateTime date)
        {
            var events = _eventService.GetByExactDate(date);
            return Ok(events);
        }
    }
}
