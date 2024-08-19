namespace Shopping_Tutorial.Models
{
    public class Paginate
    {
        public int TotalItems{ get; private set; } // tổng số item
        public int PageSize { get; private set; }   //số Page/trang
        public int CurrentPage {get; private set; } //trang hiện tại
        
        public int TotalPages { get; private set; } //tổng trang
        public int StartPage { get; private set; }  //trang bắt đầu
        public int EndPage { get; private set; }    //trang kết thúc
        public Paginate() { }
        public Paginate(int totalItems, int page, int pageSize)
        {
            int totalPages = (int)Math.Ceiling((decimal)totalItems/(decimal)pageSize);  //tính số trang và làm tròn lên 3.3=4 

            int currentPage = page; //trang hiện tại = page truyền vào

            int startPage=currentPage-5;    //trang bắt đầu - 5 button 
            int endPage=currentPage+4;      //trang kết thúc + 4 button 

            if(startPage <=0)
            {   //nếu trang bắt đầu <=0 trang cuối bằng
                endPage = endPage - (startPage - 1);    //6-(-3-1)=10  
                startPage = 1;                                                
            }
            if(endPage > totalPages)    //nếu trang cuối > tổng số trang
            {
                endPage = totalPages;   //trang cuối bằng tổng số trang     
                if(endPage >10)         //nếu trang cuối > 10               
                {
                    startPage = endPage - 9;    //trang bắt đầu = trang cuối -9 
                }
            }
            // gán vô tại thuộc tính
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
    }
}
