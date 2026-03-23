const Key = "recent_products";
const Max_Items = 10;

type product =
{
    id: number;
    productImage: string;
    name: string;
    description: string;
    price: number;
}

export const GetRecentProducts = (): product[] => {
    const stored = localStorage.getItem(Key);
    return stored ? JSON.parse(stored) : [];
};

export const AddRecentProducts = (prod: product) => {
    const stored = localStorage.getItem(Key);
    let recent: product[] = stored ? JSON.parse(stored) : [];

    recent = recent.filter(product => product.id !== prod.id);

    recent.unshift(prod);

    if(recent.length > Max_Items)
    {
        recent.pop();
    }

    localStorage.setItem(Key, JSON.stringify(recent));
};