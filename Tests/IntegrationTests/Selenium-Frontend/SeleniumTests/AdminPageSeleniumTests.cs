using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

public class AdminPageSeleniumTests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly string _baseUrl = "http://localhost:5173/#";
    private readonly string _adminEmail = "johndoes@gmail.com";
    private readonly string _adminPassword = "johndoes";
    private readonly string _userEmail = "johndoesnt@gmail.com";
    private readonly string _userPassword = "johndoesnt";

    public AdminPageSeleniumTests()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        _driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);
    }

    private void Login(string email, string password)
    {
        _driver.Navigate().GoToUrl($"{_baseUrl}/auth");
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.FindElement(By.CssSelector("input[type='email']")).Displayed);
        _driver.FindElement(By.CssSelector("input[type='email']")).SendKeys(email);
        _driver.FindElement(By.CssSelector("input[type='password']")).SendKeys(password);
        _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        Thread.Sleep(3000);
        _driver.Navigate().GoToUrl($"{_baseUrl}/");
        Thread.Sleep(1000);
    }

    [Fact]
    public void AdminPage_RedirectsToAuth_WhenNotLoggedIn()
    {
        _driver.Navigate().GoToUrl($"{_baseUrl}/admin");
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.Url.Contains("/auth") || d.Url.Contains("/admin"));
        Assert.Contains("/auth", _driver.Url);
    }

    [Fact]
    public void AuthPage_ShowsErrorMessage_WithWrongCredentials()
    {
        _driver.Navigate().GoToUrl($"{_baseUrl}/auth");
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.FindElement(By.CssSelector("input[type='email']")).Displayed);
        _driver.FindElement(By.CssSelector("input[type='email']")).SendKeys("wrong@test.com");
        _driver.FindElement(By.CssSelector("input[type='password']")).SendKeys("wrongpassword");
        _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        wait.Until(d => d.FindElement(By.CssSelector(".auth-error")).Displayed);
        Assert.True(_driver.FindElement(By.CssSelector(".auth-error")).Displayed);
    }

    [Fact]
    public void ForgotPasswordPage_ShowsForm()
    {
        _driver.Navigate().GoToUrl($"{_baseUrl}/forgot-password");
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.FindElement(By.CssSelector("input[type='email']")).Displayed);
        Assert.True(_driver.FindElement(By.CssSelector("input[type='email']")).Displayed);
    }

    [Fact]
    public void RegisterForm_IsVisible_OnAuthPage()
    {
        _driver.Navigate().GoToUrl($"{_baseUrl}/auth");
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.FindElement(By.CssSelector(".auth-tab")).Displayed);
        var registerTab = _driver.FindElements(By.CssSelector(".auth-tab"))[1];
        registerTab.Click();
        wait.Until(d => d.FindElement(By.CssSelector("input[type='email']")).Displayed);
        Assert.True(_driver.FindElement(By.CssSelector("input[type='email']")).Displayed);
    }

    [Fact]
    public void ForgotPasswordPage_ShowsSuccessMessage_AfterSubmit()
    {
        _driver.Navigate().GoToUrl($"{_baseUrl}/forgot-password");
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.FindElement(By.CssSelector("input[type='email']")).Displayed);
        _driver.FindElement(By.CssSelector("input[type='email']")).SendKeys("test@test.com");
        _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        wait.Until(d => d.FindElement(By.CssSelector(".auth-success")).Displayed);
        Assert.True(_driver.FindElement(By.CssSelector(".auth-success")).Displayed);
    }

    [Fact]
    public void ProductsPage_ShowsProducts()
    {
        _driver.Navigate().GoToUrl($"{_baseUrl}/");
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        wait.Until(d => d.FindElements(By.CssSelector(".Product-content")).Count > 0);
        Assert.True(_driver.FindElements(By.CssSelector(".Product-content")).Count > 0);
    }

    [Fact]
    public void NotFoundPage_IsShown_ForInvalidRoute()
    {
        _driver.Navigate().GoToUrl($"{_baseUrl}/deze-pagina-bestaat-niet");
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.FindElement(By.CssSelector(".NotFound-code")).Displayed);
        Assert.True(_driver.FindElement(By.CssSelector(".NotFound-code")).Displayed);
    }

    [Fact]
    public void Navbar_ShowsAdminButton_WhenLoggedInAsAdmin()
    {
        Login(_adminEmail, _adminPassword);
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.FindElements(By.XPath("//a[contains(text(),'Admin')]")).Count > 0);
        var adminLink = _driver.FindElement(By.XPath("//a[contains(text(),'Admin')]"));
        Assert.True(adminLink.Displayed);
    }

    [Fact]
    public void Navbar_HidesAdminButton_WhenLoggedInAsUser()
    {
        Login(_userEmail, _userPassword);
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.FindElements(By.CssSelector(".Product-content")).Count > 0);
        var adminLinks = _driver.FindElements(By.XPath("//a[contains(text(),'Admin')]"));
        Assert.Empty(adminLinks);
    }

    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}