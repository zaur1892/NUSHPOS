using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.Models;
using NUSHPOS.Services;
using System;

namespace NUSHPOS.ViewModels;

public partial class CheckDiscountViewModel : ObservableObject
{
    private readonly DiscountService _discountService;

    [ObservableProperty]
    private ObservableCollection<Discount> _discounts = new();

    public Discount? SelectedDiscount { get; private set; }

    public Action? RequestClose { get; set; }

    public CheckDiscountViewModel(DiscountService discountService)
    {
        _discountService = discountService;
    }

    public async Task InitializeAsync()
    {
        var activeDiscounts = await _discountService.GetActiveDiscountsAsync();
        Discounts = new ObservableCollection<Discount>(activeDiscounts);
    }

    [RelayCommand]
    private void SelectDiscount(Discount discount)
    {
        SelectedDiscount = discount;
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        SelectedDiscount = null;
        RequestClose?.Invoke();
    }
}
