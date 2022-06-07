using System.Collections.Generic;
using System;

namespace LoLA.WebAPIs.DataDragon.Objects
{
    public class Champions
    {
        public string type { get; set; }
        public string format { get; set; }
        public string version { get; set; }
        public Dictionary<string, Data> Data { get; set; }
    }

    public class Data
    {
        public string version { get; set; }
        public string id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string blurp { get; set; }
        public Info info = new Info();
        public Image image = new Image();
    }

    public class Info
    {
        public int attack { get; set; }
        public int defense { get; set; }
        public int magic { get; set; }
        public int difficulty { get; set; }
    }

    public class Image
    {
        public string full { get; set; }
        public string sprite { get; set; }
        public string group { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int w { get; set; }
        public int h { get; set; }
    }
}
