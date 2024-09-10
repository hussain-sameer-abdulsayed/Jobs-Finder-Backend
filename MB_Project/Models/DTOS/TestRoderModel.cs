namespace MB_Project.Models.DTOS
{
    public class TestRoderModel
    {
        /*
         key={order.id}
            id={order.id}
            mainImage={order.work.mainImage} *******
            title={order.work.title} *********
            description={order.description}
            basePrice={order.work.basePrice} **********
            freelancerId={order.freelancerId}
            createdAt={order.createdAt}
            status={order.status}
            totalPrice={order.totalPrice}
        */

        public int Id { get; set; }
        public string Description { get; set; }
        public Post post { get; set; }
        public string FreelancerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public double TotalPrice { get; set; }
    }
}
