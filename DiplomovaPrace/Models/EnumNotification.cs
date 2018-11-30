using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiplomovaPrace.Models
{
    public enum EnumNotification
    {
      CREATE_ACTOR, EDIT_ACTOR, DELETE_ACTOR,
      CREATE_REQUIREMENT, EDIT_REQUIREMENT, DELETE_REQUIREMENT,
      CREATE_USECASE, EDIT_USECASE,DELETE_USECASE,
      CREATE_SCENARIO, EDIT_SCENARIO, DELETE_SCENARIO,
      CREATE_FILE, DELETE_FILE, CREATE_TASK, DELETE_TASK, CHANGE_TASK
    }
}