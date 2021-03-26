using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MvvmCross.ViewModels;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;

namespace Aldentea.Idaten11.Views
{
  /// <summary>
  /// Idaten11View.xaml の相互作用ロジック
  /// </summary>
  public partial class Idaten11View : MvvmCross.Platforms.Wpf.Views.MvxWpfView
  {
    public Idaten11View()
    {
      InitializeComponent();

      var set = this.CreateBindingSet<Idaten11View, Core.ViewModels.Idaten11ViewModel>();
      set.Bind(this).For(view => view.InputScoreInteraction).To(vm => vm.InputScoreInteraction).OneWay();
      set.Apply();
    }

    public IMvxInteraction<Core.InputScoreQuestion> InputScoreInteraction
    {
      get => _inputScoreInteraction;
      set
      {
        if (_inputScoreInteraction != null)
        {
          _inputScoreInteraction.Requested -= OnInputScoreInteractionRequested;
        }
        _inputScoreInteraction = value;
        if (value != null)
        _inputScoreInteraction.Requested += OnInputScoreInteractionRequested;
      }
    }
    IMvxInteraction<Core.InputScoreQuestion> _inputScoreInteraction;

    void OnInputScoreInteractionRequested(object sender, MvxValueEventArgs<Core.InputScoreQuestion> eventArgs)
    {
      var question = eventArgs.Value;
      var vm = new Core.ViewModels.MatchScoreViewModel
      {
        Player1Name = question.Player1Name,
        Player2Name = question.Player2Name,
        // とりあえず0にしておく。
        Player1Score = 0,
        Player2Score = 0
      };
      var dialog = new MatchScoreDialog { DataContext = vm };

      var status = dialog.ShowDialog();
      question.InputScoreCallback(new Core.InputScoreAnswer
      {
        Ok = status == true,
        Player1Score = vm.Player1Score,
        Player2Score = vm.Player2Score
      });
    }

  }
}