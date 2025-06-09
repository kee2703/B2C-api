using RotaryAdminAPI.Data;
using RotaryAdminAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RotaryAdminAPI.Services
{
    public class EventService
    {
        private readonly AppDbContext _context;

        public EventService(AppDbContext context)
        {
            _context = context;
        }

        public List<Event> GetAll() => _context.Events.ToList();

        public Event? GetById(int id) => _context.Events.FirstOrDefault(e => e.Id == id);

        public Event Add(Event newEvent)
        {
            newEvent.CreatedAt = DateTime.Now;
            newEvent.UpdatedAt = DateTime.Now;

            _context.Events.Add(newEvent);
            _context.SaveChanges();

            return newEvent;
        }

        public bool Update(int id, Event updated)
        {
            var existing = _context.Events.FirstOrDefault(e => e.Id == id);
            if (existing == null) return false;

            existing.Title = updated.Title;
            existing.Location = updated.Location;
            existing.Date = updated.Date;
            existing.Time = updated.Time;
            existing.Description = updated.Description;
            existing.ActiveFlag = updated.ActiveFlag;
            existing.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var ev = _context.Events.FirstOrDefault(e => e.Id == id);
            if (ev == null) return false;

            _context.Events.Remove(ev);
            _context.SaveChanges();
            return true;
        }
        public IEnumerable<Event> GetByExactDate(DateTime date)
        {
            return _context.Events.Where(e => e.Date.Date == date.Date).ToList();
        }
    }
}
