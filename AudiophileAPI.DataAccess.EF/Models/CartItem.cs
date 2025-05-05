using System;
using System.Collections.Generic;

namespace AudiophileAPI.DataAccess.EF.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int UsersId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User Users { get; set; } = null!;
}
