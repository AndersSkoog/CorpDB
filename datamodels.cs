using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace corpdb
{

    public class subject
    {
        public string _id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string ssnr { get; set; }
        public string role { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string password { get; set; }
    }
    public struct subjectindex {
        public string email;
        public string role;
        public subjectindex(subject s) {
            email = s.email;
            role = s.role;
        }
    }
}




















    //public struct clientdata {
    //    public string email;
    //    public string role;
    //    //public string accesstoken;
    //    public clientdata(dynamic data) {
    //        email = data.email.ToString();
    //        role = data.role.ToString();
    //        //daccesstoken = data.accesstoken.ToString();
    //    }
    //}

    //public struct subjectindex {
    //    public string email;
    //    public string role;
    //    public subjectindex(dynamic data) {
    //        email = data.email.ToString();
    //        role = data.role.ToString();
    //    }
    //}

    //public struct subject {
    //    public string firstname;
    //    public string lastname;
    //    public string role;
    //    public string ssnr;
    //    public string street;
    //    public string city;
    //    public string zip;
    //    public string email;
    //    public string phone;
    //    public subject(dynamic data)
    //    {
    //        firstname = data.firstname.ToString();
    //        lastname = data.lastname.ToString();
    //        role = data.role.ToString();
    //        ssnr = data.ssnr.ToString();
    //        street = data.street.ToString();
    //        city = data.city.ToString();
    //        zip = data.zip.ToString();
    //        email = data.email.ToString();
    //        phone = data.phone.ToString();
    //    }
    //}

    //public class indexviewmodel {
    //    public List<subjectindex> Subjects;
    //    public clientdata Clientdata;
    //    public indexviewmodel(List<string> coll, dynamic clientcookie) {
    //        Clientdata = new clientdata(clientcookie);
    //        Subjects = new List<subjectindex>();
    //        foreach (string doc in coll) {
    //            dynamic d = JsonConvert.DeserializeObject(doc);
    //            Subjects.Add(new subjectindex(d));
    //        }
    //    }

    //}

    //public class subjectviewmodel {
    //    public subject Subject;
    //    public clientdata Clientdata;
    //    public bool writeable;
    //    public subjectviewmodel(dynamic _subjectdata, dynamic _clientdata) {
    //        Subject = new subject(_subjectdata);
    //        Clientdata = new clientdata(_clientdata);
    //        var WriteAccess = helpers.writeaccess(Clientdata.role);
    //        writeable = (WriteAccess.Contains(Subject.role))? true : false;

    //    }
    //}

