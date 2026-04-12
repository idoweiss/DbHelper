using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbHelperConsoleApp
{


    internal class PostService
    {

        static PostService()
        {
            DbHelper.RunSqlChange(@"
            CREATE TABLE IF NOT EXISTS Posts (
                Id INTEGER PRIMARY KEY,
                Text TEXT NOT NULL,
                Likes INTEGER DEFAULT 0,
                UserId INTEGER NOT NULL,
                FOREIGN KEY(UserId) REFERENCES Users(Id)
            );");

            //DbHelper.RunSqlChange(@"
            //    INSERT OR IGNORE INTO Posts (Text, Likes, UserId) VALUES 
            //    ('Starting our first database lesson today', 10, 1),
            //    ('Who wants to play football during the break?', 5, 1),
            //    ('Summer vacation is almost here', 50, 3),
            //    ('My cat is sleeping on the keyboard again', 15, 3),
            //    ('What a beautiful day outside', 8, 1),
            //    ('Did you see the game last night?', 30, 3),
            //    ('I need some help with loops', 4, 1),
            //    ('Who wants to study for the test together?', 7, 3),
            //    ('The new movie I saw was amazing', 25, 1),
            //    ('Finished all my tasks for today', 18, 3),
            //    ('Does anyone want to trade stickers?', 1, 1),
            //    ('My guitar is out of tune', 6, 3),
            //    ('It is incredibly hot today', 9, 1)
            //");

        }



        public List<Post> getAllPosts()
        {
            return DbHelper.RunSelect<Post>("select * from Posts");
        }

        public List<Post> getViralPosts()
        {
            return DbHelper.RunSelect<Post>("select * from Posts where Likes>10");
        }

        public List<PostWithAuthor> getViralPostsWithAuthor()
        {
            string sql = @"
                SELECT Posts.Text, Posts.Likes,  Users.UserName 
                FROM Posts 
                JOIN Users ON Posts.UserId = Users.Id where likes>10";
            return DbHelper.RunSelect<PostWithAuthor>(sql);
        }
    }
}
