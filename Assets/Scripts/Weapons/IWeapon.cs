public interface IWeapon
{
    public string WeaponName { get; set; }

    public void Initialize();
    public void fire();
}