由于今天的版本看起来很有可能成功，所以具体记录一下


//之前记得有过一个报错，直接把2.0的 emailsender类拿过来了
1、新建一个emailsender.cs 

    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }
    }

2、修改startup.cs  

//主要的部分   AddDefaultIdentity改成 Addidentity，  增加    services.AddTransient<IEmailSender, EmailSender>(); 这里就定位到刚才创建的emailsender


       services.AddIdentity<IdentityUser, IdentityRole>(options => {
                //指定密码长度为6位
                options.Password.RequiredLength = 6;
                //是否要求有非字母数字的字符
                options.Password.RequireNonAlphanumeric = false;
                //是否要求有大写的ASCII字母
                options.Password.RequireUppercase = false;
                //是否要求有小写的ASCII字母
                options.Password.RequireLowercase = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddTransient<IEmailSender, EmailSender>();


3、添加基架，否则这时候的注册和登陆页面完全不能用

//这两个控制器和视图也是从2.0抄来的
4、创建 UserAdmin和RoleAdmin的控制器和视图（目前Ilogger没法用） 

//从2.0处抄来的
5、会有用到EditUserViewModel这个类  用来给用户分配角色
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }
    }

6、这个时候已经可以给账号分配角色了，比如我们新建一个角色叫做“管理员”  那么在一个特定的action上写
        [Authorize(Roles = "管理员")]
	就可以让管理员角色之外的用户被咔嚓掉，但是还有bug，此时2.1的拒绝访问被跳转到了 /account/accessdenied  而实际上
的页面应该是/Identity/Account/AccessDenied 


//摘自官方文档，并没有任何人说应该抄这里的，我只是在官方文档中发现了 "/Identity/Account/AccessDenied"; 这个字段，所以我感觉他应该
//可以解决我的问题，然后没想到让我蒙对了
7、修改startup.cs
增加：
 services.ConfigureApplicationCookie(options =>
    {
        // Cookie settings
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

        options.LoginPath = "/Identity/Account/Login";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        options.SlidingExpiration = true;
    });





          