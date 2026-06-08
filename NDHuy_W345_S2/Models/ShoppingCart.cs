namespace NDHuy_W345_S2.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }
        public void RemoveItem(int productId) 
        {
            Items.RemoveAll(i => i.ProductId == productId);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
            if(existingItem != null) 
            {
                if(quantity <= 0)
                {
                    Items.RemoveAll(i => i.ProductId == productId);
                }
                else 
                {
                    existingItem.Quantity = quantity;
                }
            }
        }

        public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);

        public int TotalItems => Items.Sum(i => i.Quantity);
    }
}
