using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp1.Class
{
    class Part
    {
        public string partId;
        public int colour;
        public int quantity;
        public int numPhotos;

    public Part(string pId, int c, int q, int n)
        {
            partId = pId;
            colour = c;
            quantity = q;
            numPhotos = n;
        }
    }
}
