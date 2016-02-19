using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArduinoMonitor.Objects;

namespace ArduinoWeb.Models
{
    public class ArduinosViewModel
    {
        public List<Arduino> Arduinos;

        public ArduinosViewModel(List<Arduino> list)
        {
            Arduinos = list;
        }
    }
}