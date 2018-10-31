���ڽ���İ汾���������п��ܳɹ������Ծ����¼һ��


//֮ǰ�ǵ��й�һ������ֱ�Ӱ�2.0�� emailsender���ù�����
1���½�һ��emailsender.cs 

    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }
    }

2���޸�startup.cs  

//��Ҫ�Ĳ���   AddDefaultIdentity�ĳ� Addidentity��  ����    services.AddTransient<IEmailSender, EmailSender>(); ����Ͷ�λ���ղŴ�����emailsender


       services.AddIdentity<IdentityUser, IdentityRole>(options => {
                //ָ�����볤��Ϊ6λ
                options.Password.RequiredLength = 6;
                //�Ƿ�Ҫ���з���ĸ���ֵ��ַ�
                options.Password.RequireNonAlphanumeric = false;
                //�Ƿ�Ҫ���д�д��ASCII��ĸ
                options.Password.RequireUppercase = false;
                //�Ƿ�Ҫ����Сд��ASCII��ĸ
                options.Password.RequireLowercase = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddTransient<IEmailSender, EmailSender>();


3����ӻ��ܣ�������ʱ���ע��͵�½ҳ����ȫ������

//����������������ͼҲ�Ǵ�2.0������
4������ UserAdmin��RoleAdmin�Ŀ���������ͼ��ĿǰIloggerû���ã� 

//��2.0��������
5�������õ�EditUserViewModel�����  �������û������ɫ
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }
    }

6�����ʱ���Ѿ����Ը��˺ŷ����ɫ�ˣ����������½�һ����ɫ����������Ա��  ��ô��һ���ض���action��д
        [Authorize(Roles = "����Ա")]
	�Ϳ����ù���Ա��ɫ֮����û�������������ǻ���bug����ʱ2.1�ľܾ����ʱ���ת���� /account/accessdenied  ��ʵ����
��ҳ��Ӧ����/Identity/Account/AccessDenied 


//ժ�Թٷ��ĵ�����û���κ���˵Ӧ�ó�����ģ���ֻ���ڹٷ��ĵ��з����� "/Identity/Account/AccessDenied"; ����ֶΣ������Ҹо���Ӧ��
//���Խ���ҵ����⣬Ȼ��û�뵽�����ɶ���
7���޸�startup.cs
���ӣ�
 services.ConfigureApplicationCookie(options =>
    {
        // Cookie settings
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

        options.LoginPath = "/Identity/Account/Login";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        options.SlidingExpiration = true;
    });





          