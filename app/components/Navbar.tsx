import Link from "next/link";
import { CiBookmarkCheck } from "react-icons/ci";
import { CiCirclePlus } from "react-icons/ci";
interface NavbarProps {
  OpenModal: () => void;
}

const Navbar = ({ OpenModal }: NavbarProps) => {
  return (
    <nav className=" flex justify-between p-4 border-b mb-4 items-center">
      <p className="text-xl font-bold">My Library</p>
      <ul className="flex gap-5">
        <li className=" border p-2 rounded-lg hover:bg-green-400 font-semibold">
          <button
            onClick={OpenModal}
            className="flex justify-center items-center gap-1 hover:scale-110"
          >
            <CiCirclePlus size={20} /> Add Book
          </button>
        </li>
        <li className=" border p-2 rounded-lg hover:bg-green-400 font-semibold hover:scale-110">
          <Link
            className="flex justify-center items-center gap-1"
            href="/favorits"
          >
            {" "}
            <CiBookmarkCheck size={20} /> Favorits
          </Link>
        </li>
      </ul>
    </nav>
  );
};
export default Navbar;
