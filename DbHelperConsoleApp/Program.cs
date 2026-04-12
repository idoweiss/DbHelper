using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using DbHelperConsoleApp;

class Program
{
    static void Main(string[] args)
    {

        UserService userService = new UserService();
     
        //טעינת המשתתפים מבסיס הנתונים לתוך רשימה
        List<User> users = userService.GetAllUsers();
        //הדפסתת המשתתפים לקונסול
        Console.WriteLine("Users in Database:\n");
        for (int i = 0; i < users.Count; i++)
        {
            User user = users[i];
            Console.WriteLine($" {user.Id}  |  {user.FullName} |  {user.UserName} |  {user.Email}");
         }


        ////מחיקת משתמש עם מזהה 2
        //userService.DeleteUser(2);

        ////טעינת המשתתפים מבסיס הנתונים לתוך רשימה שוב לאחר המחיקה
        //users = userService.GetAllUsers();
        ////הדפסתת המשתמשים לקונסול
        //Console.WriteLine("\n\n\nUsers in Database after Delete:\n");
        //for (int i = 0; i < users.Count; i++)
        //{
        //    User user = users[i];
        //    Console.WriteLine($" {user.Id}  |  {user.FullName} |  {user.UserName} |  {user.Email}");
        //}

        ////עדכון כתובת המייל של נועה על פי האימייל הקיים שלה  
        //userService.UpdateEmailByEmail("noa@example.com", "noa@gmail.com");

        ////טעינת המשתתפים מבסיס הנתונים לתוך רשימה שוב לאחר העדכון
        //users = userService.GetAllUsers();
        ////הדפסתת המשתמשים לקונסול
        //Console.WriteLine("\n\n\nUsers in Database after Update:\n");
        //for (int i = 0; i < users.Count; i++)
        //{
        //    User user = users[i];
        //    Console.WriteLine($" {user.Id}  |  {user.FullName} |  {user.UserName} |  {user.Email}");
        //}
        PostService postService = new PostService();
        List<Post> posts = postService.getViralPosts();
        Console.WriteLine("\n\nViral Posts in Database:\n");

        for (int i = 0;i < posts.Count; i++)
        {
            Post post = posts[i];
            Console.WriteLine($" {post.Id}  |  {post.Text} |  {post.UserId} |  {post.Likes}");
        }

        List<PostWithAuthor> postsWithAuthors = postService.getViralPostsWithAuthor();
        Console.WriteLine("\n\nViral Posts With Authors in Database:\n");

        foreach (PostWithAuthor item in postsWithAuthors) 
        {
            Console.WriteLine($"  {item.Text} |  {item.UserName} |  {item.Likes}");
        }




    }
}


