
using Nancy;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;


namespace corpdb
{
    public class loginmodule : NancyModule {

        public loginmodule() {

            Get["/login"] = _ =>
            {
                return View["loginview.sshtml", new {msg = ""}];
            };
            Post["/login", runAsync: true] = async (_, token) =>
            {
                var email = Request.Form.email.ToString();
                var pw = Request.Form.pw.ToString();
                var query = await DB.Query((s) => s.email == email && s.password == utils.hasher(pw));
                if (query.Any())
                {
                    var doc = query.First();
                    string id = doc._id;
                    Session["oid"] = id;
                    utils.print(Session["oid"].ToString());
                    return Response.AsRedirect("/index");
                }
                return View["loginview.sshtml",new {msg = "invalid credentials!"}];
            };
        } 
    }

    public class user_routes : NancyModule {
        public subject user;
        public user_routes() {
            Before += async(_,token) =>
            {
                if (Session["oid"] != null)
                {
                    var oid = Session["oid"].ToString();
                    var q = await DB.Query((s) => s._id == oid);
                    if (q.Any()) { user = q.First(); return null; }
                    else { return Response.AsRedirect("/login"); }
                }
                else {return Response.AsRedirect("/login");}
            };
            Get["/"] = _ => { return Response.AsRedirect("/index"); };

            Get["/index", runAsync: true] = async(_,token) =>
            {
                var coll = await DB.allsubjects();
                var viewmodel = new { msg = "welcome " + user.firstname, Subjects = new List<subjectindex>() };
                foreach (subject s in coll) { viewmodel.Subjects.Add(new subjectindex(s)); }
                return View["indexview.sshtml", viewmodel];
            };
            Get["/signout"] = _ =>
            {
                Session.Delete("oid");
                return Response.AsRedirect("/");
            }; 

            Get["/subject/{email}", runAsync: true] = async (parameters,token) =>
            {
                var query = await Task.Run(() => DB.Query((s) => s.email == parameters.email));
                if (query.Any())
                {
                    var subject = query.First();
                    bool _writeable = (utils.haswriteaccess(user.role, subject.role) || subject.email == user.email) ? true : false;
                    utils.print(subject._id);
                    var viewmodel = new { Subject = subject, writeable = _writeable};
                    return View["subjectview.sshtml", viewmodel];
                }
                else
                {
                    return Response.AsRedirect("/index");
                }
            };
            Get["/subject/new"] = _ =>
            {
                if (user.role != "shareholder")
                {
                    return View["newsubjectview", new { roleoptions = utils.writeaccess(user.role) }];
                }
                else {
                    return Response.AsRedirect("/index");
                }
            };
            Get["/subject/edit/{email}", runAsync: true] = async (parameters, token) =>
            {
                var query = await DB.Query((s) => s.email == parameters.email.ToString());
                if (query.Any())
                {
                    var subject = query.First();
                    if (utils.haswriteaccess(user.role, subject.role) || subject.email == user.email) { return View["subjecteditview", new { Subject = subject, roleoptions = utils.writeaccess(user.role)}]; }
                }
                return Response.AsRedirect("/index");
            };
            Post["/subject/create", runAsync: true] = async (_, token) =>
            {
                dynamic s = JsonConvert.DeserializeObject(Request.Body.ReadAsString());
                var doc = new BsonDocument
                    {
                        {"firstname",s.firstname.ToString()},
                        {"lastname",s.lastname.ToString()},
                        {"role",s.role.ToString()},
                        {"ssnr",s.ssnr.ToString()},
                        {"email",s.email.ToString()},
                        {"phone",s.phone.ToString()},
                        {"street",s.street.ToString()},
                        {"city",s.city.ToString()},
                        {"zip",s.zip.ToString()}
                    };
                if (s.role.ToString() != "customer") {
                    var generatedpassword = utils.randstr(10, 16);
                    utils.print(generatedpassword);
                    var pw = utils.hasher(generatedpassword);
                    doc.Add("password", pw);
                }
                await DB.coll.InsertOneAsync(doc);
                return Response.AsText("/index");
            };
            Post["/subject/update", runAsync: true] = async (p, token) =>
            {
                var s = JsonConvert.DeserializeObject<subject>(Request.Body.ReadAsString());
                utils.print(s.firstname);
                var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(s._id));
                var update = Builders<BsonDocument>.Update
                    .Set("firstname", s.firstname)
                    .Set("lastname", s.lastname)
                    .Set("role", s.role)
                    .Set("ssnr", s.ssnr)
                    .Set("email", s.email)
                    .Set("phone", s.phone)
                    .Set("street", s.street)
                    .Set("city", s.city)
                    .Set("zip", s.zip);
                var result = await DB.coll.UpdateOneAsync(filter, update);
                return Response.AsText("/index");
            };
            Get["/subject/delete/{email}", runAsync: true] = async (parameters, token) =>
            {
                var query = await DB.Query((s) => s.email == parameters.email);
                if (query.Any()) {
                    var doc = query.First();
                    if (utils.haswriteaccess(user.role, doc.role))
                    {
                        var result = await DB.coll.DeleteOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(doc._id)));
                    }
                }
                return Response.AsRedirect("/");
            };

        }
    }
}