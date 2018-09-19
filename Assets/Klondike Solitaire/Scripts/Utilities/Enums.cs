using UnityEngine;
using System.Collections;

public class Enums : MonoBehaviour {
	public enum CardColor { clubs = 0, diamonds = 1, hearts = 2, spades = 3 };
	public enum CardValue { Ace = 1,Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10, Jack = 11, Queen = 12, King = 13};
	public enum MoveOptions { waste2tableau, waste2foundation, tableau2foundation, foundation2tableau, tableau2tableau, foundation2foundation };
	public enum ScoringType { none, normal, vegas };
	public enum LerpType { hermite, sinerp, coserp, berp, clerp,  lerp };         

}
