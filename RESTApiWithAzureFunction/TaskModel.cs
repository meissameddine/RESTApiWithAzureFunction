using System;
using System.Collections.Generic;
using System.Text;

namespace RESTApiWithAzureFunction
{
    class TaskModel
    {
    
            public int EvaxId { get; set; }
            public int cin { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int tel { get; set; }
        }


    public class CreateTaskModel
    {

        public int EvaxId { get; set; }
        public int cin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int tel { get; set; }

    }


}
