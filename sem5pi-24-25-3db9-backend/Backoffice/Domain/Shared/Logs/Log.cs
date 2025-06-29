using System;
using System.Diagnostics.Eventing.Reader;
using Backoffice.Domain.Logs;
using Backoffice.Domain.Shared;
using Backoffice.Infraestructure.Logs;

namespace Backoffice.Domain.Logs
{
    public class Log : Entity<LogId>
    {

        public new int Id { get; private set; }
        public DateTime DateTime { get; private set; }
        public string Description { get; private set; }
        public LogType LogType { get; private set; }
        public LogEntity LogEntity { get; private set; }
        public EntityId EntityId { get; private set; }

        public Log()
        {

        }


        public Log(string description, LogType logType, LogEntity logEntity, EntityId entityId)
        {
            this.DateTime = DateTime.Now;
            this.Description = description;
            this.LogType = logType;
            this.LogEntity = logEntity;
            this.EntityId = entityId;
        }
    }
}