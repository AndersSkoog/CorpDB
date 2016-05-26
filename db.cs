using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace corpdb
{

    public static class DB
    {
        public static String driverconnection = "mongodb://admin:nosalt@ds049854.mongolab.com:49854/corpdb";
        public static MongoClient mongoclient = new MongoClient(driverconnection);
        public static IMongoDatabase db = mongoclient.GetDatabase("corpdb");
        public static IMongoCollection<BsonDocument> coll = db.GetCollection<BsonDocument>("company");
        public static async Task<IEnumerable<subject>> Query(Func<subject, bool> whereclause)
        {
            var coll = await allsubjects();
            return coll.Where(whereclause);

        }
        public static async Task<List<subject>> allsubjects()
        {
            var ret = new List<subject>();
            var filter = new BsonDocument();
            var count = 0;
            using (var cursor = await coll.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var document in batch)
                    {
                        subject p = JsonConvert.DeserializeObject<subject>(utils.validjson(document));
                        ret.Add(p);
                        count++;
                    }
                }
            }
            return ret;
        }
    }
}
