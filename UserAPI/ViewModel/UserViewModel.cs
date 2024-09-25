namespace UserAPI.ViewModel
{
    public class UserViewModel
    {
        /*
         *  電子郵件（必須為有效的格式，系統內不能重複）
            密碼（長度不少於6，數字和英文字母混合，至少有一個數字和一個英文字母）
            姓名（必填）
            年齡（可選）
            性別（男/女）
            所在地區（見附表）
         */
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public string sex { get; set; }
        public string Area { get; set; }
    }
}
