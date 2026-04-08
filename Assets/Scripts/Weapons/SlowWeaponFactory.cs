public class SlowWeaponFactory : WeaponFactory
{

    private SlowWeapon slowWeapon;

    public override IWeapon GetWeapon()
    {
        slowWeapon = new SlowWeapon();
        slowWeapon.Initialize();
        return slowWeapon;
    }
}