namespace MauiApp2;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
public class QuizQuestion
{
    public int Id { get; set; } = 0;
    public string Question { get; set; } = "Error: Unable to load the question.";
    public string A { get; set; } = "-1";
    public string B { get; set; } = "-1";
    public string C { get; set; } = "-1";
    public string D { get; set; } = "-1";
    public string Answer { get; set; } = "-1";
    public string Image { get; set; } = "dall_e_error.jpg";
}
public partial class MainPage : ContentPage
{
    private int _count = 0;
    private int _rightAnswer = 0;

    private readonly string _defaultClr = "#3498DB";
    private readonly string _redClr = "#F44336";
    private readonly string _greenClr = "#4CAF50";
    
    private List<QuizQuestion> Question;

    public MainPage()
    {
        InitializeComponent();
        QuestionListInit();
    }
    
    // Wait for loading
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadQuestionsAsync();
    }

    // Button visibility
    private void AnswerBtnVisible(bool a, bool b, bool c, bool d)
    {
        AnswerA.IsVisible = a;
        AnswerB.IsVisible = b;
        AnswerC.IsVisible = c;
        AnswerD.IsVisible = d;
    }

    // Button enable
    private void AnswerBtnEnable(bool a, bool b, bool c, bool d)
    {
        AnswerA.IsEnabled = a;
        AnswerB.IsEnabled = b;
        AnswerC.IsEnabled = c; 
        AnswerD.IsEnabled = d;
    }

    // Button color
    private void AnswerBtnColor(string a, string b, string c, string d)
    {
        AnswerA.BackgroundColor = Color.FromArgb(a);
        AnswerB.BackgroundColor = Color.FromArgb(b);
        AnswerC.BackgroundColor = Color.FromArgb(c);
        AnswerD.BackgroundColor = Color.FromArgb(d);
    }
    
    // Labels swap
    private void AnswerSwap(string a, string b, string c, string d)
    {
        AnswerA.Text = a;
        AnswerB.Text = b;
        AnswerC.Text = c;
        AnswerD.Text = d;
    }
    
    // Start game
    public void StartGame(object sender, EventArgs e)
    {
        RemoteBtn.IsVisible = false;
        if (Question[0].Answer != "-1")
        {
            AnswerBtnEnable(true, true, true, true);
            SwapData();
        }
        else
        {
            System.Environment.Exit(-1);
        }
    }

    // Game reset
    public void GameRst()
    {
        _rightAnswer = 0;
        _count = 0;
        
        GameImage.Source = Question[0].Image;
        GameQuestion.Text = Question[0].Question;
        RemoteBtn.Text = Question[0].A;
        
        RemoteBtn.IsVisible = true;
        NextBtn.IsVisible = false;
        AnswerBtnVisible(false,false,false,false);
    }
    
    // Swap data
    public void SwapData()
    {
        _count++;
        GameQuestion.Text = Question[_count].Question;
        GameImage.Source = Question[_count].Image;
        
        if (Question[_count].Id == (Question.Count - 2))
        {
            AnswerBtnVisible(false,false,false,false);
            NextBtn.IsVisible = true;
            GameResult.IsVisible = true;
            GameResult.Text = (_rightAnswer == (Question.Count - 3)) ?
                Question[_count].A + $"\n\nScore: {_rightAnswer}/{Question.Count - 3}" :
                Question[_count].B + $"\n\nScore: {_rightAnswer}/{Question.Count - 3}";
            
        }
        else if (Question[_count].Id == (Question.Count - 1))
        {
            AnswerSwap(Question[_count].A, Question[_count].B, Question[_count].C, Question[_count].D);
            AnswerBtnVisible(true,true,false,false);
        }
        else
        {
            AnswerSwap(Question[_count].A, Question[_count].B, Question[_count].C, Question[_count].D);
            AnswerBtnVisible(true,true,true,true);
        }
    }
    
    // Check answer
    public void CheckAnswer(object sender, EventArgs e)
    {
        NextBtn.IsVisible = true;
        Button clickedButton = sender as Button;

        if (clickedButton != null)
        {
            AnswerBtnEnable(false, false, false, false);
            
            if (clickedButton == AnswerA)
            {
                if (Question[_count].Id == Question.Count - 1)
                {
                    GameRst();
                    return;
                }
                
                if (Question[_count].Answer == "A") _rightAnswer++;
                AnswerA.BackgroundColor = 
                    Question[_count].Answer == "A" ? 
                        Color.FromArgb(_greenClr) : 
                        Color.FromArgb(_redClr);
            }
            if (clickedButton == AnswerB)
            {
                if (Question[_count].Id == (Question.Count - 1)) System.Environment.Exit(0);
                
                if (Question[_count].Answer == "B") _rightAnswer++;
                AnswerB.BackgroundColor = 
                    Question[_count].Answer == "B" ? 
                        Color.FromArgb(_greenClr) : 
                        Color.FromArgb(_redClr);
            }
            if (clickedButton == AnswerC)
            {
                if (Question[_count].Answer == "C") _rightAnswer++;
                AnswerC.BackgroundColor = 
                    Question[_count].Answer == "C" ? 
                        Color.FromArgb(_greenClr) : 
                        Color.FromArgb(_redClr);
            }
            if (clickedButton == AnswerD)
            {
                if (Question[_count].Answer == "D") _rightAnswer++;
                AnswerD.BackgroundColor = 
                    Question[_count].Answer == "D" ? 
                        Color.FromArgb(_greenClr) : 
                        Color.FromArgb(_redClr);
            }
        }
    }

    // Next Button 
    public void NextBtnPressed(object sender, EventArgs e)
    {
        NextBtn.IsVisible = false;
        GameResult.IsVisible = false;
        AnswerBtnEnable(true, true, true, true);
        AnswerBtnColor(_defaultClr, _defaultClr, _defaultClr, _defaultClr);
        SwapData();
    }

    // Read JSON
    private async void QuestionListInit()
    {
        Question = await LoadQuestionsAsync();
        GameImage.Source = Question[0].Image;
        GameQuestion.Text = Question[0].Question;
        RemoteBtn.Text = Question[0].A;
    }
    async Task<List<QuizQuestion>> LoadQuestionsAsync()
    {
        try
        {
            await using var stream = await FileSystem.OpenAppPackageFileAsync("quiz.json");
            using var reader = new StreamReader(stream);
            var jsonContents = reader.ReadToEnd();
            
            var questionList = JsonSerializer.Deserialize<List<QuizQuestion>>(jsonContents);
            return questionList ?? new List<QuizQuestion>(Question);
        }
        catch (FileNotFoundException ex)
        {
            return new List<QuizQuestion>(Question);
        }
    }
}