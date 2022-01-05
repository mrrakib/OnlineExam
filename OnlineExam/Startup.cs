using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using OnlineExam.Data;
using OnlineExam.Models;
using Owin;

[assembly: OwinStartupAttribute(typeof(OnlineExam.Startup))]
namespace OnlineExam
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
        }

        // In this method we will create default User roles and Admin user for login
        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // In Startup I'm creating first Admin Role and creating a default Admin User     
            if (!roleManager.RoleExists("Admin"))
            {
                // first we create Admin rool    
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website
                var user = new ApplicationUser();
                user.UserName = "01771775944";
                user.PhoneNumber = "01771775944";
                user.Email = "biplob@gmail.com";

                string userPWD = "123456";
                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin    
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                }

                var shohag = new ApplicationUser();
                shohag.UserName = "01926029000";
                shohag.PhoneNumber = "01926029000";
                shohag.Email = "eshohag@outlook.com";

                var shohagAdmin = UserManager.Create(shohag, userPWD);
                //Add default User to Role Admin    
                if (shohagAdmin.Succeeded)
                {
                    var resultShohagAdmin = UserManager.AddToRole(shohag.Id, "Admin");
                }
            }

            // creating Creating Manager role     
            if (!roleManager.RoleExists("Student"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Student";
                roleManager.Create(role);
            }
        }
    }
}
