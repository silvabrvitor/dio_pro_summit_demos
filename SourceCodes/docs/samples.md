# Sample Data

Create Product
--------------

```json
{
  "name": "Surface Pro 7 256GB",
  "active": true,
  "stock": 100,
  "price": 12000,
  "images": [
    "https://cdn.pocket-lint.com/r/s/1200x/assets/images/152014-laptops-review-microsoft-surface-pro-7-still-the-best-still-no-thunderbolt-image1-khevcs2qro.jpg"
  ],
  "reviews": [
    {
      "Title": "Horrivel",
      "Content": "Produto péssimo, não tem qualidade nenhuma"
    }
  ]
}
```

Update Product with Reviews
---------------------------

```json
{
  "name": "Surface Pro 7 512GB",
  "active": true,
  "stock": 100,
  "price": 12000,
  "images": [
    "https://cdn.pocket-lint.com/r/s/1200x/assets/images/152014-laptops-review-microsoft-surface-pro-7-still-the-best-still-no-thunderbolt-image1-khevcs2qro.jpg"
  ],
  "reviews": [
      {
        "title": "Muito ruim",
        "content": "O produto é de péssima construção e não representa o valor que eu esperava"
      },
      {
        "title": "Muito bom",
        "content": "O produto é excelente, possui um acabamento muit bom e durabilidade. Adorei a cor do produto e a textura da capa do teclado."
      }
  ]
}
```